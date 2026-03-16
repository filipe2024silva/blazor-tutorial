namespace f10.pulsar.mes 
{
    public class ReceitaStepsDTO
    {
        public int? Num { get; set; }
        public string Designacao { get; set; } = string.Empty;
        public string T1 { get; set; } = string.Empty;  
        public string T2 { get; set; } = string.Empty;
        public string T1min { get; set; } = string.Empty;
        public string T1max { get; set; } = string.Empty;
        public string T2min { get; set; } = string.Empty;
        public string T2max { get; set; } = string.Empty;
        public string Opcao2 { get; set; } = string.Empty;
        public int Opcao3 { get; set; } = 0;
        public float D1 { get; set; } = 0.0f;
        public string CorBanho { get; set; } = string.Empty;    
        public float Opcao5 { get; set; } = 0.0f;
        public bool? LM { get; set; }
        public bool? SM { get; set; }

    }

}


