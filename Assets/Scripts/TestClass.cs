using Base.Helper;
using Base.Module;
using FileHelpers;
using UnityEngine;

public class TestClass : BaseMono
{
    [DelimitedRecord(",")]
    public record TestCsv
    {
        public string ID;
        public int Interger;
        public float Float;
    }
    protected override void Start()
    {
        base.Start();

        var asset = Resources.Load<TextAsset>("TestCSV/testCSV");
        FileUtilities.ReadFromCsv<TestCsv>(asset.bytes);
    } 
}
