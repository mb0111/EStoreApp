namespace EStore.API.Service.Helpers
{
    public class Output<T>
    {
        #region Private Prop

        private EnResultStatus _status;

        private string _message;

        private T _result;

        #endregion

        #region Public Prop

        public EnResultStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public T Result
        {
            get { return _result; }
            set { _result = value; }
        }

        #endregion
    }
}
