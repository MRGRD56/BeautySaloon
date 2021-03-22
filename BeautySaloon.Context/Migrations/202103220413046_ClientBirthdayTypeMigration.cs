namespace BeautySaloon.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientBirthdayTypeMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "Birthday", c => c.DateTime());
        }
    }
}
