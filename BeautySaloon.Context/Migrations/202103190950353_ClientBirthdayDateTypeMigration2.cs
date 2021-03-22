namespace BeautySaloon.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientBirthdayDateTypeMigration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime(storeType: "date"));
        }
    }
}
