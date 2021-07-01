using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfFormsApp
{
    class TpropertyRepository
    {
        CE_Context ce_context;
        public TpropertyRepository(CE_Context context)
        {
            ce_context = context;
        }
        public void CreateTpropertyEntity(string name, string value, int group_id)
        {
            if (ce_context == null) return;
            var group_id_e = ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == group_id);
            if (group_id_e == null)
            {
                MessageBox.Show($@"Объект с id = {group_id} не найден!");
                return;
            }
            var newEntity = new CE_Tproperty()
            { Name = name, Value = value, Group_id = group_id };
            ce_context.CE_Tproperty_Property.Add(newEntity);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при создании объекта Tproperty с названием " + name);
        }

        public void DeleteTpropertyEntity(int id)
        {
            if (ce_context == null) return;
            var TgpropertyEntityForDelete = ce_context.CE_Tproperty_Property.SingleOrDefault(x => x.Id == id);
            if (TgpropertyEntityForDelete == null)
            {
                MessageBox.Show($@"Объект с id = {id} не найден!");
                return;
            }
            ce_context.CE_Tproperty_Property.Remove(TgpropertyEntityForDelete);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при удалении объекта TPROPERTY с названием " + TgpropertyEntityForDelete.Name);
        }

        public void UpdateTpropertyEntity(int id, string name, string value)
        {
            if (ce_context == null) return;
            var TpropertyEntityForUpdate = ce_context.CE_Tproperty_Property.SingleOrDefault(x => x.Id == id);
            if (TpropertyEntityForUpdate == null)
            {
                MessageBox.Show($@"Объект с id = {id}, предназначенный для обновления, не найден!");
                return;
            }
            TpropertyEntityForUpdate.Name = name;
            TpropertyEntityForUpdate.Value = value;
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show(@"Возникли ошибки при обновлении объекта TPROPERTY с названием " + TpropertyEntityForUpdate.Name);
        }
        public CE_Tproperty GetTpropertyById(int id)
        {
            return ce_context.CE_Tproperty_Property.SingleOrDefault(x => x.Id == id);
        }
        public CE_Tproperty GetTpropertyByIndex(int index)
        {
            return GetTproperyInList()[index];
        }
        public List<CE_Tproperty> GetTproperyInList()
        {
            return ce_context.CE_Tproperty_Property.ToList();
        }
    }
}
