using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public bool SetPerson(string person)
        {
            var imgX = AppVars.ImgPanel[0].Img;
            if (!imgX.Person.Equals(person))
            {
                imgX.Person = person;

                var size = GetPersonSize(imgX.Person);

                Img[] scope;
                if (string.IsNullOrEmpty(imgX.Person) || size == 1)
                {
                    scope = _imgList
                        .Where(e => !e.Value.Name.Equals(imgX.Name))
                        .Select(e => e.Value)
                        .ToArray();
                }
                else
                {
                    scope = _imgList
                        .Where(e => !e.Value.Name.Equals(imgX.Name) && e.Value.Person.Equals(imgX.Person))
                        .Select(e => e.Value)
                        .ToArray();
                }

                var imgY = scope[new Random().Next(scope.Length)];
                imgX.NextName = imgY.Name;
                imgX.Distance = imgY.PHash != 0 ? HelperDescriptors.GetDistance(imgX.PHash, imgY.PHash) : 64;
                imgX.Sim = imgY.Orbs.Rows > 0 ? HelperDescriptors.GetSim(imgX.Orbs, imgY.Orbs) : 0f;
                imgX.LastChecked = GetMinLastChecked();
                imgX.LastId = 0;
                UpdateNameNext(imgX);

                AppVars.ImgPanel[1] = GetImgPanel(imgY.Name);
                return true;
            }
            
            return false;
        }

        public bool CopyRightPerson()
        {
            var imgX = AppVars.ImgPanel[0].Img;
            var imgY = AppVars.ImgPanel[1].Img;
            if (!imgX.Person.Equals(imgY.Person))
            {
                imgX.Person = imgY.Person;
                return true;
            }

            return false;
        }

        public int GetPersonSize(string person)
        {
            var personsize = _imgList.Count(e => e.Value.Person.Equals(person));
            return personsize;
        }
    }
}
