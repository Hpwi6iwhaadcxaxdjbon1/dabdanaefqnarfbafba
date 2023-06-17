using System;
using Rust;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class DevImpactEffectTest : MonoBehaviour
{
	// Token: 0x04000E6F RID: 3695
	private float timeNextFire;

	// Token: 0x04000E70 RID: 3696
	private int attackType;

	// Token: 0x060011C9 RID: 4553 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x00075A10 File Offset: 0x00073C10
	private void OnGUI()
	{
		string[] array = new string[]
		{
			"Bullet",
			"Blunt",
			"Slash"
		};
		this.attackType = GUI.SelectionGrid(new Rect(10f, 10f, 200f, 100f), this.attackType, array, 1);
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x00075A68 File Offset: 0x00073C68
	private void Update()
	{
		Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1024f);
		RaycastHit hitInfo;
		if (!Physics.Raycast(MainCamera.mainCamera.ScreenPointToRay(pos), ref hitInfo, 1024f, 1269916417))
		{
			return;
		}
		if (Buttons.Attack.IsDown)
		{
			this.DoAction(hitInfo);
		}
		if (Buttons.Attack2.IsDown && this.timeNextFire < Time.time)
		{
			this.DoAction(hitInfo);
			this.timeNextFire = Time.time + 0.1f;
		}
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x00075AF8 File Offset: 0x00073CF8
	private void DoAction(RaycastHit hitInfo)
	{
		PhysicMaterial materialAt = hitInfo.collider.GetMaterialAt(hitInfo.point);
		if (materialAt == null)
		{
			DDraw.Text("No Phys Material", hitInfo.point, Color.red, 2f);
			return;
		}
		DamageType damageType = DamageType.Bullet;
		if (this.attackType == 1)
		{
			damageType = DamageType.Blunt;
		}
		if (this.attackType == 2)
		{
			damageType = DamageType.Slash;
		}
		string particle = EffectDictionary.GetParticle(damageType, materialAt.name);
		if (particle == "")
		{
			DDraw.Text("Missing: " + materialAt.name, hitInfo.point, Color.green, 2f);
			return;
		}
		Effect.client.Run(particle, hitInfo.point, hitInfo.normal, default(Vector3));
	}
}
