namespace FloidUI
{
    public partial class MainPage
    {
        public class EntriesGrid
        {
            Entry[,] entries;
            int size;
            Grid grid;

            public Grid GetGrid { get => grid; }
            
            public EntriesGrid(int size, int dimensions) 
            {
                this.size = size;
                entries = new Entry[size, size];
                GenerateGrid(dimensions);
            }

            public void GenerateGrid(int dimensions)
            {
                Grid grid = new Grid()
                {
                    BackgroundColor = Colors.Black
                };

                for (int i = 0; i < size; i++)
                {
                    grid.AddColumnDefinition(new ColumnDefinition());
                    grid.AddRowDefinition(new RowDefinition());
                }

                for (int i = 0; i < size; i++)
                { 
                    for (int j = 0; j < size; j++)
                    {
                        var temp = new Entry()
                        { 
                            BackgroundColor = Colors.White,
                            Margin = 2,
                            HeightRequest = dimensions / size - 4,
                            WidthRequest = dimensions / size - 4,
                            FontSize = dimensions / (size*3),
                        };
                        entries[i, j] = temp;
                        grid.Add(temp, i, j);
                    }
                }

                this.grid = grid;
            }
        }
    }

}
