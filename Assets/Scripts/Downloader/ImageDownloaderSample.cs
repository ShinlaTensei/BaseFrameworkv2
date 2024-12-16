using System;
using Base.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ImageDownloaderSample : MonoBehaviour
{
    [SerializeField] private string m_url;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private SpriteRenderer m_spriteRenderer1;
    [SerializeField] private SpriteRenderer m_spriteRenderer2;

    private Texture2D m_texture2D;
    // Start is called before the first frame update
    void Start()
    {
        ImageDownloader.Create().Load(m_url).AttachToken(this.GetCancellationTokenOnDestroy())
            .WithCompleted(OnLoadTextureCompleted).Start();
    }

    private void OnLoadTextureCompleted(Texture2D texture2D)
    {
        if (texture2D != null) m_texture2D = texture2D;
        
        Sprite sprite = Sprite.Create(m_texture2D, new Rect(0, 0, m_texture2D.width, m_texture2D.height), new Vector2(0.5f, 0.5f));
        if (sprite == null) return;

        m_spriteRenderer.sprite = sprite;
        m_spriteRenderer1.sprite = sprite;
        m_spriteRenderer2.sprite = sprite;
    }

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
