﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public enum ParameterType { IN, OUT, IN_OUT }
    public class DBParameter
    {
        string name;
        string typeOfValue;
        object value;
        ParameterType sqlType;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string TypeOfValue
        {
            get
            {
                return typeOfValue;
            }

            set
            {
                typeOfValue = value;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public ParameterType SqlType
        {
            get
            {
                return sqlType;
            }

            set
            {
                sqlType = value;
            }
        }

        public DBParameter(string name, string typeOfValue, object value, ParameterType sqlType = ParameterType.IN) {
            this.name = name;
            this.typeOfValue = typeOfValue;
            this.value = value;
            this.sqlType = sqlType;
        }
    }
}
