namespace ProjektCona1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Podatkis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPostaje = c.Int(nullable: false),
                        Cas = c.DateTime(nullable: false),
                        Temp = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Vlaga = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Nekaj = c.String(),
                        Nevem = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Podatkis");
        }
    }
}
