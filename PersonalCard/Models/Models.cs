using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PersonalCard.Blockchain;

namespace PersonalCard.Models
{
    public class SmartContractModel
    {
        [Required]
        public string hash_сustomer;
        [Required]
        public string hash_еxecutor;
        [Required]
        public string order_sum;
        [Required]
        public string prepaid_expense;
        [Required]
        public string condition;
        public bool is_Done;
        public bool is_freze;
    }

    public class LoginModel
    {
        [Required]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string code_phrase { get; set; }

        [Required]
        public string type_of_blood { get; set; }


    }

    public class MedicalModel
    {
        [Required]
        public string diagnosis { get; set; }
        [Required]
        public string diagnosis_fully { get; set; }
        [Required]
        public string first_aid { get; set; }
        [Required]
        public string drugs { get; set; }
        [Required]
        public bool is_important { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public string key_frase { get; set; }
    }

	public class MedicalsModel
	{
		public List<Medical> medicals { get; set; }

	}

    public class ApiModel
    {
        
        [Required]
        public string site { get; set; }
        [Required]
        public string organisation { get; set; }
    }
}
