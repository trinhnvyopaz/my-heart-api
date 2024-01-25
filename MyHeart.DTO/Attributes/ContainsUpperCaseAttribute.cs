using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.Attributes
{
    public class ContainsUpperCaseAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return false;
            }

            if (value is string str)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (Char.IsUpper(str[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
