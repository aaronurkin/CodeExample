namespace AaronUrkinCodeExample.DataAccessLayer.Localization.Entities
{
    public class Translation
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Scope { get; set; }

        public string Value { get; set; }

        public string CultureCode { get; set; }
    }
}
