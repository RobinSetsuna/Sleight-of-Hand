using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

    private List<Renderer> _ObstacleCollider;


    public GameObject _target;

  
    private Renderer _tempRenderer;
    void Start()
    {
        _ObstacleCollider = new List<Renderer>();
    }
    void Update()
    {
  
#if UNITY_EDITOR
        Debug.DrawLine(_target.transform.position, transform.position, Color.red);
#endif
        RaycastHit[] hit;
        hit = Physics.RaycastAll(_target.transform.position, transform.position);
        //  
        if (hit.Length > 0)
        {   
            for (int i = 0; i < hit.Length; i++)
            {
                _tempRenderer = hit[i].collider.gameObject.GetComponent<Renderer>();
                _ObstacleCollider.Add(_tempRenderer);
                SetMaterialsAlpha(_tempRenderer, 0.5f);
                Debug.Log(hit[i].collider.name);
            }


        }
        else
        {
            for (int i = 0; i < _ObstacleCollider.Count; i++)
            {
                _tempRenderer = _ObstacleCollider[i];
                SetMaterialsAlpha(_tempRenderer, 1f);
            }
        }

    }



    private void SetMaterialsAlpha(Renderer _renderer, float Transpa)
    {

        int materialsCount = _renderer.materials.Length;
        for (int i = 0; i < materialsCount; i++)
        {


            Color color = _renderer.materials[i].color;

 
            color.a = Transpa;

            _renderer.materials[i].SetColor("_Color", color);
        }

    } 

}
