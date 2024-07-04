using Base.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ImageDownloaderSample : MonoBehaviour
{
    [SerializeField] private string m_url;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private SpriteRenderer m_spriteRenderer1;
    [SerializeField] private SpriteRenderer m_spriteRenderer2;
    // Start is called before the first frame update
    void Start()
    {
        ImageDownloader.Create().Load(m_url).Into(m_spriteRenderer).AttachToken(this.GetCancellationTokenOnDestroy()).Start();
        ImageDownloader.Create().Load(m_url).Into(m_spriteRenderer1).AttachToken(this.GetCancellationTokenOnDestroy()).Start();
        ImageDownloader.Create().Load(m_url).Into(m_spriteRenderer2).AttachToken(this.GetCancellationTokenOnDestroy()).Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
