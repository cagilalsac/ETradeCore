namespace AppCore.Records.Bases
{
    public abstract class RecordBase // ilişki entity'leri dışında tüm entity'lerin ve model'lerin miras alacağı ve veritabanındaki entity'lerin karşılığı olan tablolarda sütunları oluşacak özellikler
    {
        public int Id { get; set; }
        public string? Guid { get; set; } // Guid zorunlu olmasın
    }
}
