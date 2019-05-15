namespace TommoJProductions.ModApi.Attachable.Options
{
    /// <summary>
    /// Represents types of initialization procedures.
    /// </summary>
    public enum PartInitializationProcedureEnum
    {
        /// <summary>
        /// Represents a procedure that attempts to find the gameobject by name and then instantiates new instance of the gameobject if failed.
        /// </summary>
        FindThenInstantiate,
        /// <summary>
        /// Represents a procedure to instantiate the gameobject.
        /// </summary>
        Instantiate,        
        /// <summary>
        /// Represents a procedure to find the gameobject.
        /// </summary>
        Find,
        /// <summary>
        /// Represets a procedure to assign the gameobject.
        /// </summary>
        Assign
    }
}
