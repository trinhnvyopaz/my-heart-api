using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation {
    public class AtcDTO : BaseEntityDTO {
        public string AtcCode { get; set; }
        public string NT { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public List<MedicamentGroupDTO> MedicamentGroups { get; set; }

        public static AtcDTO FromCsv(string csv) {
            var values = csv.Split(';');

            var atc = new AtcDTO {
                AtcCode = values[0],
                NT = values[1],
                Name = values[2],
                NameEn = values[3]
            };

            atc.Id = atc.GetCodeHash();

            return atc;
        }

        public int GetCodeHash() {
            return GetAtcCodeHash(AtcCode);
        }

        public static int GetAtcCodeHash(string code) {
            var sb = new StringBuilder();

            var split = code.ToCharArray();

            sb.Append((FirstLetter[split[0]]).ToString("00"));

            if (split.Length >= 3) {
                sb.Append(split[1]);
                sb.Append(split[2]);
            }

            if (split.Length >= 4) {
                sb.Append((split[3] - 64).ToString("00"));
            }

            if (split.Length >= 5) {
                sb.Append((split[4] - 64).ToString("00"));
            }

            if (split.Length >= 7) {
                sb.Append(split[5]);
                sb.Append(split[6]);
            }

            if (int.TryParse(sb.ToString(), out var j)) {
                return j;
            }
            return 0;
        }

        public override bool Equals(object obj) {
            return obj is AtcDTO d &&
                   this.Id == d.Id &&
                   this.LastUpdateDate == d.LastUpdateDate &&
                   this.LastUpdateUser == d.LastUpdateUser &&
                   this.AtcCode == d.AtcCode &&
                   this.NT == d.NT &&
                   this.Name == d.Name &&
                   this.NameEn == d.NameEn;
        }

        public bool EqualsWithoutBaseEntity(object obj) {
            return obj is AtcDTO d &&
                   this.AtcCode == d.AtcCode &&
                   this.NT == d.NT &&
                   this.Name == d.Name &&
                   this.NameEn == d.NameEn;
        }

        private static Dictionary<char, int> FirstLetter = new Dictionary<char, int> {
            {'A', 1},
            {'B', 2},
            {'C', 3},
            {'D', 4},
            {'G', 5},
            {'H', 6},
            {'J', 7},
            {'L', 8},
            {'M', 9},
            {'N', 10},
            {'P', 11},
            {'R', 12},
            {'S', 13},
            {'V', 14},
        };
    }
}
