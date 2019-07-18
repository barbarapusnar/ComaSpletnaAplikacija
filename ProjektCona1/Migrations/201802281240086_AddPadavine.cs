namespace ProjektCona1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPadavine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Podatkis", "Padavine", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Podatkis", "Padavine");
        }
    }
}
