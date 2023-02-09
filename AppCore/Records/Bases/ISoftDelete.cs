namespace AppCore.Records.Bases
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        // istenilen entity'lere bu interface implemente edilerek kayıtların veritabanındaki tablolarında silinmesi yerine
        // bu kayıtların tutularak silindi olarak işaretlenmesi sağlanabilir,
        // böylece bu kayıtlar istenirse SQL Server Management Studio üzerinden sorgulanabilir
    }
}
