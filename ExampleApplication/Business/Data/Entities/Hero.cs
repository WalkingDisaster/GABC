using System;
using System.Configuration;
using System.Linq;
using ExampleApplication.Business.Data.Framework;
using Microsoft.Azure.Documents;

namespace ExampleApplication.Business.Data.Entities
{
    [Document("HeroDb", typeof(Hero))]
    public class Hero : Document
    {
        public string Name
        {
            get { return GetPropertyValue<string>("name"); }
            set { SetPropertyValue("name", value);}
        }

        public string AlsoKnownAs
        {
            get { return GetPropertyValue<string>("alsoKnownAs"); }
            set { SetPropertyValue("alsoKnownAs", value);}
        }

        public DispatchType DispatchStatus
        {
            get
            {
                var stringResult = GetPropertyValue<string>("dispatchStatus");
                var parsed = (DispatchType)Enum.Parse(typeof(DispatchType), stringResult);
                return parsed;

            }
            set { SetPropertyValue("dispatchStatus", value.ToString()); }
        }

        public string DispatchLocation
        {
            get { return GetPropertyValue<string>("dispatchLocation"); }
            set { SetPropertyValue("dispatchLocation", value); }
        }

        public HeroTypes Types
        {
            get
            {
                var stringResult = GetPropertyValue<string>("types");
                var parsed = (HeroTypes) Enum.Parse(typeof (HeroTypes), stringResult);
                return parsed;
            }
            set
            {
                SetPropertyValue("types", value.ToString());
            }
        }

        public Weapon[] Weapons
        {
            get { return GetPropertyValue<Weapon[]>("weapons"); }
            set { SetPropertyValue("weapons", value); }
        }
    }
}