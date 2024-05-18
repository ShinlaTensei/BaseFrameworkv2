using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base.Helper;
using UnityEngine;

public class SaveTest : BaseMono
{
    [SerializeField] private bool m_isSave = true;
    protected override void Start()
    {
        if (m_isSave)
        {
            using (var stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.WriteLine("This is a test data");
                }
            }
        }
        else
        {
            using (var stream = new MemoryStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    string data = sr.ReadToEnd();
                }
            }
        }
    }
}
