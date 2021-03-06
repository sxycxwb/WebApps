﻿using Apps.Common;
using Apps.Models;
using Apps.Models.Spl;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Spl.BLL
{
    public partial class Spl_PersonBLL
    {
        /// <summary>
        /// 校验Excel数据
        /// </summary>
        public bool CheckImportData(string fileName, List<Spl_PersonModel> personList,ref ValidationErrors errors )
        {
          
            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {

                errors.Add("导入的数据文件不存在");
                return false;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //对应列头
            excelFile.AddMapping<Spl_PersonModel>(x => x.Name, "Name");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Sex, "Sex");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Age, "Age");
            excelFile.AddMapping<Spl_PersonModel>(x => x.IDCard, "IDCard");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Phone, "Phone");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Email, "Email");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Address, "Address");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Region, "Region");
            excelFile.AddMapping<Spl_PersonModel>(x => x.Category, "Category");
            //SheetName
            var excelContent = excelFile.Worksheet<Spl_PersonModel>(0);

            int rowIndex = 1;

            //检查数据正确性
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var person = new Spl_PersonModel();

                person.Id = "";
                person.Name = row.Name;
                person.Sex = row.Sex;
                person.Age = row.Age;
                person.IDCard = row.IDCard;
                person.Phone = row.Phone;
                person.Email = row.Email;
                person.Address = row.Address;
                person.Region = row.Region;
                person.Category = row.Category;

                if (string.IsNullOrWhiteSpace(row.Name))
                {
                    errorMessage.Append("Name - 不能为空. ");
                }

                if (string.IsNullOrWhiteSpace(row.IDCard))
                {
                    errorMessage.Append("IDCard - 不能为空. ");
                }

                //=============================================================================
                if (errorMessage.Length > 0)
                {
                    errors.Add(string.Format(
                        "第 {0} 列发现错误：{1}{2}",
                        rowIndex,
                        errorMessage,
                        "<br/>"));
                }
                personList.Add(person);
                rowIndex += 1;
            }
            if (errors.Count > 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveImportData(IEnumerable<Spl_PersonModel> personList)
        {
            try
            {
                DBContainer db = new DBContainer();
                foreach (var model in personList)
                {
                    Spl_Person entity = new Spl_Person();
                    entity.Id = ResultHelper.NewId;
                    entity.Name = model.Name;
                    entity.Sex = model.Sex;
                    entity.Age = model.Age;
                    entity.IDCard = model.IDCard;
                    entity.Phone = model.Phone;
                    entity.Email = model.Email;
                    entity.Address = model.Address;
                    entity.CreateTime = ResultHelper.NowTime;
                    entity.Region = model.Region;
                    entity.Category = model.Category;
                    db.Spl_Person.Add(entity);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
