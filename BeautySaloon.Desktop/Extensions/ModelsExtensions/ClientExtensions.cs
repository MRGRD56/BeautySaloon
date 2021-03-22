using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySaloon.Context;
using BeautySaloon.Model.DbModels;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.Extensions.ModelsExtensions
{
    public static class ClientExtensions
    {
        public static void CopyPropertiesFrom(this Client client, Client anotherClient, AppContext db)
        {
            client.LastName = anotherClient.LastName;
            client.FirstName = anotherClient.FirstName;
            client.Patronymic = anotherClient.Patronymic;
            client.Email = anotherClient.Email;
            client.Phone = anotherClient.Phone;
            client.Birthday = anotherClient.Birthday;
            client.Gender = db.Genders.Find(anotherClient.Gender.Code);
            client.PhotoPath = anotherClient.PhotoPath;
        }

        public static void SetTags(this Client client, IEnumerable<Tag> tags, AppContext db)
        {
            //Клиент должен быть из db

            var tagsList = tags.ToList();
            tagsList.ForEach(tag =>
            {
                var dbTag = db.Tags.Find(tag.ID);
                //Если клиент не содержит тег, не в списке tags тег есть - добавляем тег клиенту.
                if (client.Tags.All(x => x.ID != tag.ID) && tags.Any(x => x.ID == tag.ID))
                {
                    client.Tags.Add(dbTag);
                }
            });

            client.Tags.ToList().ForEach(tag =>
            {
                //Если клиент содержит тег, но в списке tags тега нет - удаляем тег у клиента.
                if (client.Tags.Any(x => x.ID == tag.ID) && tags.All(x => x.ID != tag.ID))
                {
                    var clientTag = client.Tags.First(x => x.ID == tag.ID);
                    client.Tags.Remove(clientTag);
                }
            });

            db.SaveChanges();
        }

        public static void SetTagsNoDb(this Client client, IEnumerable<Tag> tags)
        {
            var tagsList = tags.ToList();
            tagsList.ForEach(tag =>
            {
                //Если клиент не содержит тег, не в списке tags тег есть - добавляем тег клиенту.
                if (client.Tags.All(x => x.ID != tag.ID) && tags.Any(x => x.ID == tag.ID))
                {
                    client.Tags.Add(tag);
                }
            });

            client.Tags.ToList().ForEach(tag =>
            {
                //Если клиент содержит тег, но в списке tags тега нет - удаляем тег у клиента.
                if (client.Tags.Any(x => x.ID == tag.ID) && tags.All(x => x.ID != tag.ID))
                {
                    var clientTag = client.Tags.First(x => x.ID == tag.ID);
                    client.Tags.Remove(clientTag);
                }
            });
        }
    }
}
