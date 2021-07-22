using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TreeViewWpfDB
{
    class TgroupRepository
    {
        CE_Context ce_context;
        public TgroupRepository(CE_Context context)
        {
            ce_context = context;
        }
        public void CreateTgroupEntity(string name)
        {
            if (ce_context == null) return;
            var newEntity = new CE_Tgroup()
            { Name = name };
            ce_context.CE_Tgroup_Property.Add(newEntity);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при создании объекта TGROUP с названием " + name);
        }
        //удаляет так же все дочерние группы и отношения между ними
        public void DeleteTgroupEntity(int id)
        {
            if (ce_context == null) return;
            var TgroupEntityForDelete = ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == id);
            DeleteTgroupWithChildrenAndProperties(id);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при удалении объекта TGROUP с названием " + TgroupEntityForDelete.Name);
        }
        void DeleteTgroupWithChildrenAndProperties(int id)
        {
            var tGroups = ce_context.CE_Tgroup_Property.ToList();
            var tRelations = ce_context.CE_Trelation_Property.ToList();
            var tPropreties = ce_context.CE_Tproperty_Property.ToList();
            DeleteTgroupWithChildrenAndProperties(id, ref tGroups, ref tRelations, ref tPropreties);
        }
        //рекурсивный метод, который удаляет группу вместе со всеми группами и свойствами, которые привязаны к ней
        void DeleteTgroupWithChildrenAndProperties(int id, ref List<CE_Tgroup> tGroups, ref List<CE_Trelation> tRelations, ref List<CE_Tproperty> tPropreties)
        {
            if (tGroups.Find(x => x.Id == id) == null)
            {
                MessageBox.Show($@"Объект с id = {id} не найден!");
                return;
            }
            if (tPropreties.Find(x => x.Group_id == id) != null)
            {
                foreach (var prop in tPropreties.FindAll(x => x.Group_id == id))
                {
                    ce_context.CE_Tproperty_Property.Remove(prop);
                    ce_context.SaveChanges();
                    tPropreties.Remove(prop);
                }
            }
            if (tRelations.Find(x => x.Id_parent == id) != null)
            {
                foreach (var rel in tRelations.FindAll(x => x.Id_parent == id))
                    DeleteTgroupWithChildrenAndProperties(rel.Id_child, ref tGroups, ref tRelations, ref tPropreties);
            }
            if (tRelations.Find(x => x.Id_parent == id) == null)
            {
                var delGroup = tGroups.Find(x => x.Id == id);
                var delRelation = tRelations.Find(x => x.Id_child == id);
                if (delRelation != null)
                {
                    ce_context.CE_Trelation_Property.Remove(delRelation);
                    ce_context.SaveChanges();
                }
                ce_context.CE_Tgroup_Property.Remove(delGroup);
                ce_context.SaveChanges();
                tRelations.Remove(delRelation);
                tGroups.Remove(delGroup);
            }

        }
        public void UpdateTgroupEntity(int id, string name)
        {
            if (ce_context == null) return;
            var TgroupEntityForUpdate = ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == id);
            if (TgroupEntityForUpdate == null)
            {
                MessageBox.Show($@"Объект с id = {id}, предназначенный для обновления, не найден!");
                return;
            }
            TgroupEntityForUpdate.Name = name;
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при обновлении объекта TGROUP с названием " + TgroupEntityForUpdate.Name);
        }
        public CE_Tgroup GetTgroupById(int id)
        {
            return ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == id);
        }
        public CE_Tgroup GetTgroupByIndex(int index)
        {
            return GetTgroupsInList()[index];
        }
        public List<CE_Tgroup> GetTgroupsInList()
        {
            return ce_context.CE_Tgroup_Property.ToList();
        }
    }
}
