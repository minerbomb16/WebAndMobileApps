using System.ComponentModel;

namespace aplikacjaMobilna.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        private bool _isCompleted;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
