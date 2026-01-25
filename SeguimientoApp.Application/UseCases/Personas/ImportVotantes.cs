using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Domain.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class ImportVotantes(IPersonaRepositoryPort personaRepository)
    {
        private readonly IPersonaRepositoryPort _personaRepository = personaRepository;
        private const int TIPO_DOCUMENTO_CC = 1;

        public async Task<ImportVotantesResult> ExecuteAsync(Stream csvStream, CancellationToken ct)
        {
            var rows = new List<ImportVotanteRowResult>();

            // 1️) Leer CSV
            string content;
            using var reader = new StreamReader(csvStream, Encoding.Latin1, true);
            content = await reader.ReadToEndAsync(ct);

            if (string.IsNullOrWhiteSpace(content))
                return Fail("El archivo CSV está vacío.");

            var lines = content
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(l => l.Trim())
                .ToList();

            // 2️) Obtener documento del líder
            var documentoLider = ExtraerDocumentoLider(lines);
            if (documentoLider is null)
                return Fail("No se encontró el número de documento del líder.");

            // 3️) Validar líder existente
            var lider = await _personaRepository.GetByDocumentoAsync(TIPO_DOCUMENTO_CC, documentoLider.Value, ct);
            if (lider is null)
                return Fail($"El líder con documento {documentoLider} no existe.");

            var nombreLider = $"{lider.PrimerNombre} {lider.SegundoNombre} {lider.PrimerApellido} {lider.SegundoApellido}"
                .Replace("  ", " ")
                .Trim();

            // 4️) Detectar cabecera
            var headerIndex = lines.FindIndex(l => Normalizar(l).Contains("ITEM") && Normalizar(l).Contains("CC"));
            if (headerIndex < 0)
                return Fail("No se encontró la cabecera de la tabla.");

            var delimiter = DetectarSeparador(lines[headerIndex]);
            var headers = Split(lines[headerIndex], delimiter);

            int colCC = Find(headers, "CC");
            int colPN = Find(headers, "PRIMER NOMBRE");
            int colSN = Find(headers, "SEGUNDO NOMBRE");
            int colPA = Find(headers, "PRIMER APELLIDO");
            int colSA = Find(headers, "SEGUNDO APELLIDO");
            int colTel = Find(headers, "TELEFONO");
            int colDir = Find(headers, "DIRECCION");

            int creadas = 0, relacionesCreadas = 0, omitidas = 0;

            // 5️) Procesar filas
            for (int i = headerIndex + 1; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = Split(line, delimiter);
                var filaVacia = cols.All(c => string.IsNullOrWhiteSpace(c));
                if (filaVacia)
                {
                    continue;
                }

                var rawCC = Get(cols, colCC);
                if (string.IsNullOrWhiteSpace(rawCC))
                {
                    omitidas++;
                    rows.Add(new ImportVotanteRowResult(
                        Documento: null,
                        PersonaCreada: false,
                        RelacionCreada: false,
                        Status: ImportRowStatus.InvalidDocumento,
                        Motivo: "Fila sin número de documento"
                    ));
                    continue;
                }

                if (!long.TryParse(SoloNumeros(rawCC), out var cc))
                {
                    omitidas++;
                    rows.Add(new ImportVotanteRowResult(
                        Documento: null,
                        PersonaCreada: false,
                        RelacionCreada: false,
                        Status: ImportRowStatus.InvalidDocumento,
                        Motivo: $"Documento inválido: {rawCC}"
                    ));
                    continue;
                }

                bool personaCreada = false;
                long idPersona;

                var persona = await _personaRepository.GetByDocumentoAsync(TIPO_DOCUMENTO_CC, cc, ct);
                if (persona is null)
                {
                    var personaDto = new PersonaCreateDto
                    {
                        IdTipoDocumento = TIPO_DOCUMENTO_CC,
                        NumeroDocumento = cc,
                        PrimerNombre = Get(cols, colPN) ?? "",
                        SegundoNombre = Get(cols, colSN) ?? "",
                        PrimerApellido = Get(cols, colPA) ?? "",
                        SegundoApellido = Get(cols, colSA) ?? "",
                        Celular = ParseLong(Get(cols, colTel)),
                        Correo = "",
                        Direccion = Get(cols, colDir) ?? "",
                        LugarVotacion = "",
                        Mesa = "",
                        MesaNumero = null,
                        Estado = true,
                        EsLider = false,
                    };

                    await _personaRepository.CreateAsync(personaDto, ct);
                    idPersona = personaDto.IdPersona!.Value;
                    creadas++;
                    personaCreada = true;
                }
                else
                {
                    idPersona = persona.IdPersona;
                }

                var assign = await _personaRepository.AddPersonaToLiderAsync(lider.IdPersona, idPersona, ct);
                if (assign == PersonaLiderAssignResult.Ok)
                {
                    relacionesCreadas++;
                    rows.Add(new ImportVotanteRowResult(
                        Documento: cc,
                        PersonaCreada: personaCreada,
                        RelacionCreada: true,
                        Status: ImportRowStatus.Assigned,
                        Motivo: "Asignado correctamente"
                    ));
                }
                else
                {
                    omitidas++;
                    long? liderActualDoc = null;
                    string? liderActualNombre = null;

                    if (assign == PersonaLiderAssignResult.AlreadyAssigned)
                    {
                        var liderActual = await _personaRepository.GetLiderActualByPersonaIdAsync(idPersona, ct);
                        if (liderActual != null)
                        {
                            liderActualDoc = liderActual.NumeroDocumento;
                            liderActualNombre = liderActual.NombreCompleto;
                        }
                    }

                    rows.Add(new ImportVotanteRowResult(
                        Documento: cc,
                        PersonaCreada: personaCreada,
                        RelacionCreada: false,
                        Status: MapAssign(assign),
                        Motivo: MotivoAssign(assign, liderActualDoc, liderActualNombre),
                        LiderActualDocumento: liderActualDoc,
                        LiderActualNombre: liderActualNombre
                    ));
                }
            }

            return new ImportVotantesResult(
                Ok: true,
                Error: null,
                DocumentoLider: documentoLider.Value,
                NombreLider: nombreLider,
                PersonasCreadas: creadas,
                RelacionesCreadas: relacionesCreadas,
                Omitidos: omitidas,
                Rows: rows
            );
        }

        private static ImportVotantesResult Fail(string error) => new(false, error, 0, null, 0, 0, 0, new());
        private static char DetectarSeparador(string line) => line.Count(c => c == ';') >= line.Count(c => c == ',') ? ';' : ',';
        private static string[] Split(string line, char sep) => line.Split(sep).Select(x => x.Trim()).ToArray();
        private static string? Get(string[] cols, int idx) => idx >= 0 && idx < cols.Length ? cols[idx] : null;
        private static string SoloNumeros(string s) => new(s.Where(char.IsDigit).ToArray());
        private static long ParseLong(string? s) => long.TryParse(SoloNumeros(s ?? ""), out var n) ? n : 0;
        private static string Normalizar(string s) =>
            s.ToUpperInvariant()
             .Replace("Á", "A").Replace("É", "E").Replace("Í", "I")
             .Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "N");
        private static long? ExtraerDocumentoLider(IEnumerable<string> lines)
        {
            var text = Normalizar(string.Join(" ", lines.Take(15)));
            var m = Regex.Match(text, @"DOCUMENTO\s+LIDER\D*(\d+)");
            return m.Success && long.TryParse(m.Groups[1].Value, out var n) ? n : null;
        }
        private static int Find(string[] headers, string name)
        {
            var key = Normalizar(name);
            for (int i = 0; i < headers.Length; i++)
                if (Normalizar(headers[i]) == key) return i;
            return -1;
        }
        private static ImportRowStatus MapAssign(PersonaLiderAssignResult r) => r switch
        {
            PersonaLiderAssignResult.AlreadyAssigned => ImportRowStatus.AlreadyAssigned,
            PersonaLiderAssignResult.DuplicateRelation => ImportRowStatus.DuplicateRelation,
            PersonaLiderAssignResult.PersonaInactive => ImportRowStatus.PersonaInactive,
            PersonaLiderAssignResult.PersonaIsLider => ImportRowStatus.PersonaIsLider,
            PersonaLiderAssignResult.PersonaNotFound => ImportRowStatus.PersonaNotFound,
            _ => ImportRowStatus.Error
        };
        private static string MotivoAssign(PersonaLiderAssignResult r, long? docLider, string? nombreLider) => r switch
        {
            PersonaLiderAssignResult.AlreadyAssigned =>
                docLider is null
                    ? "La persona ya está asignada a otro líder"
                    : $"La persona ya está asignada a otro líder: {docLider} — {nombreLider}".Trim(),
            PersonaLiderAssignResult.DuplicateRelation => "La persona ya estaba asociada a este líder",
            PersonaLiderAssignResult.PersonaInactive => "La persona está inactiva",
            PersonaLiderAssignResult.PersonaIsLider => "La persona es líder (no se puede asignar como votante)",
            PersonaLiderAssignResult.PersonaNotFound => "La persona no existe",
            PersonaLiderAssignResult.SamePerson => "El líder no puede asignarse a sí mismo",
            _ => "Error desconocido al asignar"
        };
    }
}
