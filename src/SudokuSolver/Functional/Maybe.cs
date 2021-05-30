namespace SudokuSolver.Functional
{
    internal struct Maybe<T>
    {
        private readonly bool isValue;
        public T Value { get; }
        public bool Is => isValue;
        public bool IsNot => !isValue;

        public Maybe(bool isValue, T value)
        {
            this.isValue = isValue;
            Value = value;
        }

        public static Maybe<T> Some(T value) => new Maybe<T>(true, value);
        public static Maybe<T> None => new Maybe<T>(false, default);

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(true, value);
    }
}