using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEffect : MonoBehaviour
{
    [SerializeField] private List<Material> changeableMaterials = new List<Material>();
    [SerializeField] private float speed = 3.0f;
    private ParticleSystem particles;

    private void Start()
    {
        if (particles = gameObject.GetComponentInChildren<ParticleSystem>())
        {
            particles.gameObject.SetActive(false);
        }

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat != null && mat.name.ToLower().Contains("leaves"))
                {
                    changeableMaterials.Add(mat);
                }
            }
        }
    }

    public void SetMaterialsOpacity(float targetAmount)
    {
        //if (particles != null)
        //{
        //    if (targetAmount >= 0.7f)
        //    {
        //        ParticleSystem.MainModule main = particles.main;
        //        main.loop = false;
        //    }
        //    else
        //    {
        //        ParticleSystem.MainModule main = particles.main;
        //        main.loop = true;
        //        particles.Play();
        //    }
        //}


        StartCoroutine(ChangeMaterials(targetAmount));
    }

    private IEnumerator ChangeMaterials(float to)
    {
        float time = 0f;
        float from = changeableMaterials[0].GetFloat("_Amount");
        while (Mathf.Abs(changeableMaterials[0].GetFloat("_Amount") - to) > 0.01f)
        {
            foreach (var mat in changeableMaterials)
            {
                float To = Mathf.Lerp(from, to, time * speed);
                mat.SetFloat("_Amount", To);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in changeableMaterials)
        {
            mat.SetFloat("_Amount", to);
        }

    }
}
