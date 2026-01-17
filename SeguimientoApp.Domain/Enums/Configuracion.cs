namespace SeguimientoApp.Domain.Enums
{
    public enum TipoCatalogo
    {
        TipoDocumento = 1,
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
}
