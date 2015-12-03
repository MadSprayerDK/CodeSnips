namespace ManoSoftware.SpotifyLocalApi.Model
{
    public class Status
    {
        public int Version { set; get; }
        public string Client_Version { set; get; }
        public bool Playing { set; get; }
        public bool Shuffle { set; get; }
        public bool Repeat { set; get; }
        public bool Play_Enabled { set; get; }
        public bool Prev_Enabled { set; get; }
        public bool Next_Enabled { set; get; }
        public Track Track { set; get; }
        public object Context { set; get; }
        public float Playing_Position { set; get; }
        public double Server_Time { set; get; }
        public double Volume { set; get; }
        public bool Online { set; get; }
        public OpenGraphState Open_Graph_State { set; get; }
        public bool Running { set; get; }
    }
}
