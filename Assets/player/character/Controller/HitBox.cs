using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashingTime = 0.6f;
    private float dashingCooldown = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)&&canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "bullet" && !isDashing)
        {
            Flash();
        }
    }

}
