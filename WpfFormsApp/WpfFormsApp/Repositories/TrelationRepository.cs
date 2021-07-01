using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfFormsApp
{
    class TrelationRepository
    {
        CE_Context ce_context;
        public TrelationRepository(CE_Context context)
        {
            ce_context = context;
        }
        public void CreateTrelationEntity(int id_p, int id_c)
        {
            if (ce_context == null) return;
            var id_p_e = ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == id_p);
            if (id_p_e == null)
            {
                MessageBox.Show($@"Объект с id = {id_p} не найден!");
                return;
            }
            var id_c_e = ce_context.CE_Tgroup_Property.SingleOrDefault(x => x.Id == id_c);
            if (id_c_e == null)
            {
                MessageBox.Show($@"Объект с id = {id_c} не найден!");
                return;
            }
            var newEntity = new CE_Trelation()
            { Id_parent = id_p, Id_child = id_c };
            ce_context.CE_Trelation_Property.Add(newEntity);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show($@"Возникли ошибки при создании объекта TRELATION с айди: {id_p},{id_c}");
        }
        public void DeleteTrelationEntity(int id_p, int id_c)
        {
            if (ce_context == null) return;
            var TrelationEntityForDelete = ce_context.CE_Trelation_Property.SingleOrDefault(x => x.Id_parent == id_p && x.Id_child == id_c);
            if (TrelationEntityForDelete == null)
            {
                MessageBox.Show($@"Объект с id = {id_p},{id_c} не найден!");
                return;
            }
            ce_context.CE_Trelation_Property.Remove(TrelationEntityForDelete);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show($@"Возникли ошибки при удалении объекта TRELATION с айди: {id_p},{id_c}");
        }
        public void UpdateTrelationEntity(int id_p, int id_c, int id_p_new, int id_c_new)
        {
            DeleteTrelationEntity(id_p, id_c);
            CreateTrelationEntity(id_p_new, id_c_new);
            var res = ce_context.SaveChanges();
            if (res < 0)
                MessageBox.Show($@"Возникли ошибки при обвновлении объекта TRELATION с айди: {id_p},{id_c}");
        }
        public CE_Trelation GetTrelationByIndex(int index)
        {
            return GetTrelationInList()[index];
        }
        public List<CE_Trelation> GetTrelationInList()
        {
            return ce_context.CE_Trelation_Property.ToList();
        }
    }
}
