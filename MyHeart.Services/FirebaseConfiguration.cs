using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services
{
    public class FirebaseConfiguration : IFirebaseConfiguration
    {
        public string FireBaseType { get; set; }
        public string FirebaseProjectId { get; set; }
        public string FirebasePrivateKeyId { get; set; }
        public string FirebasePrivateKey { get; set; }
        public string FirebaseClientEmail { get; set; }
        public string FirebaseClientId { get; set; }
        public string FirebaseAuthUri { get; set; }
        public string FirebaseTokenUri { get; set; }
        public string FirebaseAuthProviderCertUrl { get; set; }
        public string FirebaseClientCertUrl { get; set; }
    }
}
