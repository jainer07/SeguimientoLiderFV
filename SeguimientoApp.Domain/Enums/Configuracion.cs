namespace SeguimientoApp.Domain.Enums
{
    public enum TipoCatalogo
    {
        TipoDocumento = 1,
        TipoEvento = 2,
        EstadoEvento = 3,
    }
    public enum ImportRowStatus
    {
        Assigned,        // se creó relación líder-persona
        AlreadyAssigned, // ya tenía líder
        DuplicateRelation,
        PersonaInactive,
        PersonaIsLider,
        PersonaNotFound,
        InvalidDocumento,
        EmptyRow,
        Error
    }
    public enum PersonaLiderAssignResult
    {
        Ok = 0,
        SamePerson,
        PersonaNotFound,
        PersonaInactive,
        PersonaIsLider,
        AlreadyAssigned,
        DuplicateRelation
    }

    public enum TipoEvento
    {
        SoloLideres = 1,
        LideresYVotantes = 2
    }

    public enum EstadoEvento
    {
        Planeado = 1,
        EnEjecucion = 2,
        Cerrado = 3,
        Anulado = 4
    }
}
