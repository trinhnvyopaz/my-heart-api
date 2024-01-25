using System;
using System.ComponentModel.DataAnnotations;

namespace MyHeart.DTO.Attributes
{
    public class ContainsLowerCaseAttribute : ValidationAttribute
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
                    if (Char.IsLower(str[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
