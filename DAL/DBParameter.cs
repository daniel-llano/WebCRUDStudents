namespace DAL
{
    public enum ParameterType { IN, OUT }
    public enum TypeOfValue { LONG, INTEGER, DECIMAL, BOOLEAN, CHAR, STRING, TEXT, DATE, DATETIME }
    public class DBParameter
    {
        string name;
        TypeOfValue typeOfValue;
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

        public TypeOfValue TypeOfValue
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

        public DBParameter(string name, TypeOfValue typeOfValue, object value, ParameterType sqlType = ParameterType.IN) {
            this.name = name;
            this.typeOfValue = typeOfValue;
            this.value = value;
            this.sqlType = sqlType;
        }
    }
}
