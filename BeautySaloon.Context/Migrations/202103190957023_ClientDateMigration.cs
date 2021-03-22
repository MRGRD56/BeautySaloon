namespace BeautySaloon.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientDateMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
    }
}
