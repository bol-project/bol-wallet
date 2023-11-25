using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Dynamic;
namespace BolWallet
{
    
    public static class CustomEditContext
    {
        public static object GetValueFromObject(this object T, string propertyName)
        {
            object res = null;
            if (T is ExpandoObject tag)
            {
                var prop = (IDictionary<string, object>)tag;
                res = prop[propertyName].ToString();
            }
            else
            {
                //var r = T.GetType().GetProperties().Single(x => x.Name == propertyName);
                var r = T.GetType().GetProperty(propertyName);
                if (r != null)
                    res = r?.GetValue(T, null) ?? "";
                else
                {
                    string json = JsonConvert.SerializeObject(T);
                    JObject jo = JsonConvert.DeserializeObject<JObject>(json);
                    if (res == null)
                        res = new List<string>();
                    foreach (var obj in jo.Children().Values())
                    {
                        var first = obj.Count() > 0 ? obj.FirstOrDefault().Values().FirstOrDefault() : null;
                        if (first != null && !(first is JValue))
                        {
                            var itm = first?[propertyName];
                            if (itm != null)
                            {
                                if (itm is JArray jitm)
                                    (res as List<string>).AddRange(jitm.ToObject<List<string>>());
                                else if (itm is JValue vitm)
                                    (res as List<string>).Add(vitm.ToString());
                            }
                        }
                    }
                    res = (res as List<string>).Distinct().ToList();
                    /*
                    var type = T.GetType().GetProperties();
                    var Implementation3 = DictionaryOfPropertiesFromInstance(T.GetType());
                    foreach (var propertyInfo in type)
                    {

                        var xrt = propertyInfo.GetType()
    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
         .ToDictionary(prop => prop.Name, prop => prop.GetValue(propertyInfo, null));

                    }
                    */
                }
            }
            return res;
        }

        private static Dictionary<string, List<string>> data = new();

        public static void SetData(this EditContext editContext, Dictionary<string, List<string>> _data)
        {
            data = _data;
        }

        public static List<string> GetData(this EditContext editContext, string customField)
        {
            if (data.ContainsKey(customField))
                return data[customField];
            else
                return new();
        }

        public static PropertyInfo GetProperty<T>(this T @this, string name)
        {
            return @this.GetType().GetProperty(name);
        }

    }
    public class CustomValidation : ComponentBase
    {
        private ValidationMessageStore messageStore;
        public Dictionary<string, List<string>> customMessages { get; set; } = new();

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }


        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CustomValidation)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(CustomValidation)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            CurrentEditContext.SetData(customMessages);

            messageStore = new(CurrentEditContext);

            CurrentEditContext.OnValidationRequested +=
                (sender, eventArgs) => OnValidationRequested(sender, eventArgs);
            CurrentEditContext.OnFieldChanged +=
                (sender, eventArgs) => OnFieldChanged(sender, eventArgs);

        }

        private async void CheckClass(object x)
        {
            if (x is BaseProperty bm)
            {
                if (bm.HasError || string.IsNullOrEmpty(bm.Value))
                {
                    bool isValid = false;
                    string errorMsg = string.Empty;
                    string val = string.Empty;
                    
                    
                    if (string.IsNullOrEmpty(bm.Value))
                    {
                        errorMsg = "Field is mandatory";
                    }

                    string fName = "";// $"{fiedName}{IDX}";
                    if (customMessages.ContainsKey(fName))
                    {
                        customMessages[fName].Add(errorMsg);
                    }
                    else
                    {
                        customMessages.Add(fName, new List<string>() { errorMsg });
                    }
                    messageStore.Add(CurrentEditContext.Field(fName), errorMsg);
                }
            }
            else
            {
                List<string> validItms = new();
                var valid = x.GetValueFromObject("ToValidate");
                if (valid != null)
                {
                    validItms = (List<string>)valid;
                }

                foreach (var y in validItms)
                {
                    bool isValid = true;
                    string errorMsg = string.Empty;
                    string val = string.Empty;
                    var o = x.GetValueFromObject(y);

                    var reqAttr = x.GetProperty(y)?.GetCustomAttributes(true).Where(x => x.GetType().Name == "RequiredAttribute").FirstOrDefault();

                    if (o is List<string> lo)
                    {
                        string s = lo[0];
                        if (s == null || string.IsNullOrEmpty(s))
                        {
                            if (reqAttr != null)
                            {
                                isValid = false;
                                errorMsg = "Field is mandatory";
                            }
                        }
                        else
                        {
                            val = s;
                        }
                    }
                    else
                    {
                        if (o == null || string.IsNullOrEmpty(o?.ToString()))
                        {
                            if (reqAttr != null)
                            {
                                isValid = false;
                                errorMsg = "Field is mandatory";
                            }
                        }
                        else
                        {
                            val = o?.ToString();
                        }
                    }

                    var attrs = x.GetProperty(y)?.GetCustomAttributes(true);
                    string fiedName = y;
                    if (attrs != null)
                    {
                        //if (attrs.Where(x => x.GetType().Name == "AliasAttribute").FirstOrDefault() != null)
                        //{
                        //    var aAttr = ((AliasAttribute)attrs.Where(x => x.GetType().Name == "AliasAttribute").FirstOrDefault());
                        //    fiedName = aAttr.AliasName;

                        //}
                        if (isValid)
                        {
                            bool RequiredIfValid = false;
                            foreach (var attr in attrs)
                            {
                                if (!isValid)
                                    break;
                                string errMsg = errMsg = "Field is mandatory";
                                switch (attr.GetType().Name)
                                {
                                    case "MinLengthAttribute":
                                        var lAttr = ((MinLengthAttribute)attr);
                                        if (val.Length < lAttr.Length && !RequiredIfValid)
                                            isValid = false;
                                        errorMsg = isValid ? errorMsg : "Minimum length"; // Loc["MinLengthChar"].ToString().Replace("%", lAttr.Length.ToString());
                                        break;
                                    case "MaxLengthAttribute":
                                        var mAttr = ((MaxLengthAttribute)attr);
                                        if (val.Length > mAttr.Length)
                                            isValid = false;
                                        errorMsg = isValid ? errorMsg : "Maximun length"; //Loc["MaxLengthChar"].ToString().Replace("%", mAttr.Length.ToString());
                                        break;

                                    case "StringLengthAttribute":
                                        var sAttr = ((StringLengthAttribute)attr);
                                        if (val.Length > sAttr.MaximumLength || val.Length < sAttr.MinimumLength)
                                        {
                                            isValid = false;
                                            errMsg = val.Length > sAttr.MaximumLength ? "String Maximum length" : "String Minimum length"; //Loc["MaxLengthChar"].ToString().Replace("%", sAttr.MaximumLength.ToString()) : Loc["MinLengthChar"].ToString().Replace("%", sAttr.MinimumLength.ToString());
                                        }
                                        errorMsg = isValid ? errorMsg : errMsg;
                                        break;

                                        //case "IsValidEmailAttribute":
                                        //    if (!string.IsNullOrEmpty(val))
                                        //    {
                                        //        if (!val.IsValidEmailAdvanced())
                                        //            isValid = false;
                                        //    }

                                        //    errorMsg = isValid ? errorMsg : Loc["EmailNotValid"];
                                        //    break;

                                        //case "OnlyLatinUpperSpaceAttribute":
                                        //    if (!string.IsNullOrEmpty(val))
                                        //    {
                                        //        if (!val.IsLatinUpperSpace())
                                        //            isValid = false;
                                        //    }

                                        //    errorMsg = isValid ? errorMsg : Loc["OnlyLatinUpper"];
                                        //    break;

                                        //case "OnlyLatinUpperDigitsAttribute":
                                        //    if (!string.IsNullOrEmpty(val))
                                        //    {
                                        //        if (!val.IsLatinUpperDigits())
                                        //            isValid = false;
                                        //    }

                                        //    errorMsg = isValid ? errorMsg : Loc["OnlyLatinDigitsUpper"];
                                        //    break;

                                        //case "OnlyLatinSpaceAttribute":
                                        //    if (!string.IsNullOrEmpty(val))
                                        //    {
                                        //        if (!val.IsLatinSpace())
                                        //            isValid = false;
                                        //    }

                                        //    errorMsg = isValid ? errorMsg : Loc["OnlyLatin"];
                                        //    break;

                                }
                            }
                        }
                    }

                    if (!isValid)
                    {
                        string str = "";
                        string IDX = string.Empty;

                        if (x.GetValueFromObject("IDX") is List<string> list)
                        {
                            str = list[0];
                        }
                        else
                        {
                            str = x.GetValueFromObject("IDX")?.ToString();
                        }
                        if (!string.IsNullOrEmpty(str))
                            IDX = "_" + str;

                        string fName = $"{fiedName}{IDX}";
                        if (customMessages.ContainsKey(fName))
                        {
                            customMessages[fName].Add(errorMsg);
                        }
                        else
                        {
                            customMessages.Add(fName, new List<string>() { errorMsg });
                        }
                        messageStore.Add(CurrentEditContext.Field(fName), errorMsg);
                    }
                }
            }
        }

        private void OnValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            messageStore.Clear();
            customMessages = new();

            var propInfo = CurrentEditContext.Model.GetType().GetProperties();

            foreach (var pp in propInfo)
            {
                if (pp.PropertyType.Name == "Dictionary`2")
                {
                    FieldIdentifier fi = CurrentEditContext.Field(pp.Name);
                    var val = fi.Model.GetProperty(pp.Name).GetValue(fi.Model);

                    if (val is System.Collections.IDictionary propInfoNested)
                    {
                        foreach (System.Collections.DictionaryEntry p in propInfoNested)
                        {
                            if (p.Value.GetType().Name == "List`1")
                            {
                                var ins = (IEnumerable<object>)p.Value;
                                foreach (var x in ins)
                                {
                                    CheckClass(x);
                                }
                            }
                            else if (p.Value.GetType().IsClass)
                            {
                                CheckClass(p.Value);
                            }
                            else
                            {

                            }
                        }

                    }
                }
                else if (pp.PropertyType.Name == "List`1")
                {
                    var fi = CurrentEditContext.Field(pp.Name);
                    var val = fi.Model.GetProperty(pp.Name).GetValue(fi.Model) as System.Collections.IEnumerable;
                    foreach (var p in val)
                    {
                        if (p.GetType().IsClass)
                        {
                            CheckClass(p);
                        }
                        else
                        {
                            var ins = (IEnumerable<object>)p;
                            foreach (var x in ins)
                            {
                                CheckClass(x);
                            }
                        }
                    }

                }
                else if (pp.GetType().IsClass)
                {
                    var fi2 = CurrentEditContext.Field(pp.Name);
                    var val = fi2.Model.GetProperty(pp.Name).GetValue(fi2.Model);
                    CheckClass(val);
                }
            }
            CurrentEditContext.SetData(customMessages);
            CurrentEditContext.NotifyValidationStateChanged();

        }

        private async void OnFieldChanged(object sender, FieldChangedEventArgs args)
        {
            FieldIdentifier fi = args.FieldIdentifier;
            List<string> validItms = new();

            object valid = fi.Model.GetValueFromObject("ToValidate");
            if (valid != null)
            {
                //messageStore.Clear(fi);
                string IDX = string.Empty;
                if (!string.IsNullOrEmpty(fi.Model.GetValueFromObject("IDX")?.ToString()))
                    IDX = "_" + fi.Model.GetValueFromObject("IDX")?.ToString();

                customMessages.Where(x => x.Key == $"{fi.FieldName}{IDX}").ToList().ForEach(x => customMessages.Remove(x.Key));

                validItms = (List<string>)valid;

                foreach (var y in validItms)
                {
                    //var attrs = x.GetProperty(y)?.GetCustomAttributes(true);
                    bool isValid = true;

                    var reqAttr = fi.Model.GetProperty(y).GetCustomAttributes(true).Where(x => x.GetType().Name == "RequiredAttribute").FirstOrDefault();
                    if (y == fi.FieldName)
                    {
                        var o = fi.Model.GetValueFromObject(y);
                        string val = o?.ToString();
                        if (o != null)
                        {
                            if (reqAttr != null)
                            {
                                if (string.IsNullOrEmpty(o?.ToString()))
                                {
                                    isValid = false;
                                    string fName = $"{y}{IDX}";
                                    if (customMessages.ContainsKey(fName))
                                    {
                                        customMessages[fName].Add("Field Is Mandatory"); //"Το πεδίο είναι υποχρεωτικό");
                                        messageStore.Add(CurrentEditContext.Field(fName), "Field Is Mandatory"); //"Το πεδίο είναι υποχρεωτικό");
                                    }
                                    else
                                    {
                                        customMessages.Add(fName, new List<string>() { "Field Is Mandatory" }); //"Το πεδίο είναι υποχρεωτικό"
                                        messageStore.Add(CurrentEditContext.Field(fName), "Field Is Mandatory"); //"Το πεδίο είναι υποχρεωτικό"
                                    }
                                }
                            }

                            if (isValid)
                            {
                                var attrs = fi.Model.GetProperty(y).GetCustomAttributes(true);
                                if (attrs != null)
                                {
                                    string fName = $"{y}{IDX}";
                                    //if (attrs.Where(x => x.GetType().Name == "AliasAttribute").FirstOrDefault() != null)
                                    //{
                                    //    var aAttr = ((AliasAttribute)attrs.Where(x => x.GetType().Name == "AliasAttribute").FirstOrDefault());
                                    //    //fiedName = aAttr.AliasName;
                                    //}

                                    if (isValid)
                                    {
                                        foreach (var attr in attrs)
                                        {
                                            switch (attr.GetType().Name)
                                            {
                                                case "MinLengthAttribute":
                                                    var lAttr = ((MinLengthAttribute)attr);
                                                    if (val.Length < lAttr.Length)
                                                    {
                                                        isValid = false;
                                                        string minErrorMsg = "Min Length Char";// Loc["MinLengthChar"].ToString().Replace("%", lAttr.Length.ToString());
                                                        if (customMessages.ContainsKey(fName))
                                                        {
                                                            customMessages[fName].Add(minErrorMsg);
                                                            messageStore.Add(CurrentEditContext.Field(fName), minErrorMsg);
                                                        }
                                                        else
                                                        {
                                                            customMessages.Add(fName, new List<string>() { minErrorMsg });
                                                            messageStore.Add(CurrentEditContext.Field(fName), minErrorMsg);
                                                        }
                                                    }
                                                    break;

                                                case "MaxLengthAttribute":
                                                    var mAttr = ((MaxLengthAttribute)attr);
                                                    if (val.Length > mAttr.Length)
                                                    {
                                                        isValid = false;
                                                        string maxErrorMsg = "Max Length Char";// Loc["MaxLengthChar"].ToString().Replace("%", mAttr.Length.ToString());
                                                        if (customMessages.ContainsKey(fName))
                                                        {
                                                            customMessages[fName].Add(maxErrorMsg);
                                                            messageStore.Add(CurrentEditContext.Field(fName), maxErrorMsg);
                                                        }
                                                        else
                                                        {
                                                            customMessages.Add(fName, new List<string>() { maxErrorMsg });
                                                            messageStore.Add(CurrentEditContext.Field(fName), maxErrorMsg);
                                                        }
                                                    }
                                                    break;

                                                case "StringLengthAttribute":
                                                    var sAttr = ((StringLengthAttribute)attr);
                                                    if (val.Length > sAttr.MaximumLength || val.Length < sAttr.MinimumLength)
                                                    {
                                                        isValid = false;
                                                        string errMsg = val.Length > sAttr.MaximumLength ? "Max Length Char" : "Min Length Char";//Loc["MaxLengthChar"].ToString().Replace("%", sAttr.MaximumLength.ToString()) : Loc["MinLengthChar"].ToString().Replace("%", sAttr.MaximumLength.ToString());
                                                        if (customMessages.ContainsKey(fName))
                                                        {
                                                            customMessages[fName].Add(errMsg);
                                                            messageStore.Add(CurrentEditContext.Field(fName), errMsg);
                                                        }
                                                        else
                                                        {
                                                            customMessages.Add(fName, new List<string>() { errMsg });
                                                            messageStore.Add(CurrentEditContext.Field(fName), errMsg);
                                                        }
                                                    }
                                                    break;

                                                //case "RequiredIfAttribute":
                                                //    var bAttr = ((RequiredIfAttribute)attr);
                                                //    var sFieldName = bAttr.OtherProperty?.ToString();
                                                    
                                                //    customMessages[fName].Add(Loc["FieldIsMandatory"]);
                                                //    messageStore.Add(CurrentEditContext.Field(fName), Loc["FieldIsMandatory"]);
                                                //    break;

                                                //case "IsValidEmailAttribute":
                                                //    if (!string.IsNullOrEmpty(val))
                                                //    {
                                                //        if (!val.IsValidEmailAdvanced())
                                                //        {
                                                //            isValid = false;
                                                //            if (customMessages.ContainsKey(fName))
                                                //            {
                                                //                customMessages[fName].Add(Loc["EmailNotValid"]);
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["EmailNotValid"]);
                                                //            }
                                                //            else
                                                //            {
                                                //                customMessages.Add(fName, new List<string>() { Loc["EmailNotValid"] });
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["EmailNotValid"]);
                                                //            }

                                                //        }
                                                //    }
                                                //    break;

                                                //case "OnlyLatinUpperSpaceAttribute":
                                                //    if (!string.IsNullOrEmpty(val))
                                                //    {
                                                //        if (!val.IsLatinUpperSpace())
                                                //        {
                                                //            isValid = false;
                                                //            if (customMessages.ContainsKey(fName))
                                                //            {
                                                //                customMessages[fName].Add(Loc["OnlyLatinUpper"]);
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatinUpper"]);
                                                //            }
                                                //            else
                                                //            {
                                                //                customMessages.Add(fName, new List<string>() { Loc["OnlyLatinUpper"] });
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatinUpper"]);
                                                //            }
                                                //        }
                                                //    }
                                                //    break;

                                                //case "OnlyLatinUpperDigitsAttribute":
                                                //    if (!string.IsNullOrEmpty(val))
                                                //    {
                                                //        if (!val.IsLatinUpperDigits())
                                                //        {
                                                //            isValid = false;
                                                //            if (customMessages.ContainsKey(fName))
                                                //            {
                                                //                customMessages[fName].Add(Loc["OnlyLatinDigitsUpper"]);
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatinDigitsUpper"]);
                                                //            }
                                                //            else
                                                //            {
                                                //                customMessages.Add(fName, new List<string>() { Loc["OnlyLatinDigitsUpper"] });
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatinDigitsUpper"]);
                                                //            }
                                                //        }
                                                //    }
                                                //    break;

                                                //case "OnlyLatinSpaceAttribute":
                                                //    if (!string.IsNullOrEmpty(val))
                                                //    {
                                                //        if (!val.IsLatinSpace())
                                                //        {
                                                //            isValid = false;
                                                //            if (customMessages.ContainsKey(fName))
                                                //            {
                                                //                customMessages[fName].Add(Loc["OnlyLatin"]);
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatin"]);
                                                //            }
                                                //            else
                                                //            {
                                                //                customMessages.Add(fName, new List<string>() { Loc["OnlyLatin"] });
                                                //                messageStore.Add(CurrentEditContext.Field(fName), Loc["OnlyLatin"]);
                                                //            }
                                                //        }
                                                //    }
                                                //    break;

                                            }
                                            if (!isValid)
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                CurrentEditContext.SetData(customMessages);
                CurrentEditContext.NotifyValidationStateChanged();
                return;
            }
            else
            {
                messageStore.Clear(fi);
                /*
                var val = fi.Model.GetProperty(fi.FieldName).GetValue(fi.Model);
                if ((fi.FieldName.Equals("FirstName") || fi.FieldName.Equals("LastName")) && (val == null || string.IsNullOrEmpty(val.ToString())) )
                {
                    messageStore.Add(CurrentEditContext.Field(fi.FieldName), "Το πεδίο είναι υποχρεωτικό");
                }
                CurrentEditContext.NotifyValidationStateChanged();
                */
            }

        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore.Clear();
            customMessages = new();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

}
