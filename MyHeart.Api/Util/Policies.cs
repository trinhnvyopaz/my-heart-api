using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Api.Util {
    public static class Policies {
        public const string MinAdmin = "Admin";
        public const string MinDoctor = "Doctor";
        public const string MinPatient = "Patient";
        public const string MinDataManager = "DataManager";
    }
}
