  Š            2018.2.1f1 ū˙˙˙      ˙˙3$øĖuņė˛e+ Í=   ^          7  ˙˙˙˙         Ļ ˛            Đ                 Ļ                Ļ                Ļ #               Ļ +               H 3   ˙˙˙˙       1  1  ˙˙˙˙   @    Ū      	        Q  j     
        H <   ˙˙˙˙       1  1  ˙˙˙˙   @    Ū              Q  j             Õ I   ˙˙˙˙       1  1  ˙˙˙˙    Ā    Ū               H j  ˙˙˙˙       1  1  ˙˙˙˙   @    Ū              Q  j              P             AssetMetaData guid data[0] data[1] data[2] data[3] pathName originalName labels assetStoreRef    ˙˙}	ôsžÕēĖ?6V;   Ę          7  ˙˙˙˙         Ļ ˛               E            Ū  #             . ,              Ä            Ū  #             . ,             H Ģ ˙˙˙˙      1  1  ˙˙˙˙	   @    Ū      
        Q  j             ņ  5   ˙˙˙˙       1  1  ˙˙˙˙        Ū                j  ˙˙˙˙        G     ˙˙˙˙        H ]   ˙˙˙˙       1  1  ˙˙˙˙   @    Ū              Q  j             H b   ˙˙˙˙       1  1  ˙˙˙˙   @    Ū              Q  j             H k   ˙˙˙˙       1  1  ˙˙˙˙   @    Ū              Q  j             y 
             Ū  #             . ,             Õ p   ˙˙˙˙        1  1  ˙˙˙˙!    Ā    Ū      "          j  ˙˙˙˙#        H   ˙˙˙˙$       1  1  ˙˙˙˙%   @    Ū      &        Q  j     '        y 
    (         Ū  #      )       . ,      *               +    @    ž       ,    @    Ū  #      -       . ,      .       H    ˙˙˙˙/       1  1  ˙˙˙˙0   @    Ū      1        Q  j     2        H Ŗ   ˙˙˙˙3       1  1  ˙˙˙˙4   @    Ū      5        Q  j     6        H ĩ   ˙˙˙˙7       1  1  ˙˙˙˙8   @    Ū      9        Q  j     :      MonoImporter PPtr<EditorExtension> m_FileID m_PathID m_ExternalObjects SourceAssetIdentifier type assembly name m_DefaultReferences executionOrder icon m_UserData m_AssetBundleName m_AssetBundleVariant s    ˙˙öčÅ7žŗĶcÖŗ÷P'   l       7  ˙˙˙˙         Ļ ˛                E            Ū               .               Ä            Ū               .              H Ģ ˙˙˙˙      1  1  ˙˙˙˙	   @    Ū      
        Q  j             H ę ˙˙˙˙      1  1  ˙˙˙˙   @    Ū              Q  j             ņ  (   ˙˙˙˙      1  1  ˙˙˙˙       Ū               j  ˙˙˙˙       H   ˙˙˙˙      1  1  ˙˙˙˙   @    Ū              Q  j             y 
            Ū               .              y <               Ū               .              Ū  C              H T   ˙˙˙˙      1  1  ˙˙˙˙    @    Ū      !        Q  j     "        H `   ˙˙˙˙#      1  1  ˙˙˙˙$   @    Ū      %        Q  j     &      PPtr<EditorExtension> m_FileID m_PathID m_DefaultReferences m_Icon m_ExecutionOrder m_ClassName m_Namespace                                   D      āy¯     Đ   ŧ                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     fŗ÷Ö,N3Ž>ķ#D+ė^   Packages/com.unity.package-manager-ui/Editor/Sources/UI/Interfaces/IPackageManagerExtension.cs                                                                                                                         IPackageManagerExtension,  using UnityEngine.Experimental.UIElements;

namespace UnityEditor.PackageManager.UI
{
    /// <summary>
    /// Interface for Package Manager UI Extension
    /// </summary>
    public interface IPackageManagerExtension
    {
        /// <summary>
        /// Creates the extension UI visual element.
        /// </summary>
        /// <returns>A visual element that represents the UI or null if none</returns>
        VisualElement CreateExtensionUI();
        
        /// <summary>
        /// Called by the Package Manager UI when the package selection changed.
        /// </summary>
        /// <param name="packageInfo">The newly selected package information (can be null)</param>
        void OnPackageSelectionChange(PackageManager.PackageInfo packageInfo);
        
        /// <summary>
        /// Called by the Package Manager UI when a package is added or updated.
        /// </summary>
        /// <param name="packageInfo">The package information</param>
        void OnPackageAddedOrUpdated(PackageManager.PackageInfo packageInfo);
        
        /// <summary>
        /// Called by the Package Manager UI when a package is removed.
        /// </summary>
        /// <param name="packageInfo">The package information</param>
        void OnPackageRemoved(PackageManager.PackageInfo packageInfo);
    }
}                       IPackageManagerExtension   UnityEditor.PackageManager.UI                                                                                                                                                                                                                                                                                                                                                                                       older != null) {
                float localHeight = jumpHeight * Mathf.Abs(Mathf.Sin(travelRatio * Mathf.PI * jumpsPerMove));
                modelHolder.transform.localPosition = Vector3.up * localHeight;
            }

            if (travelRatio >= 1) {
                speed = 0;
                break;
            }

            yield return null;
        }

        transform.position = new Vector3(destination.x, transform.position.y, destination.z);

        GridPosition = GridManager.Instance.GetTile(transform.position).gridPosition;

        actionPoint--;
        attributeSystem.AddStatusEffect(new StatusEffect(1, LevelManager.Instance.RoundNumber + 2));

        if (callback != null)
            callback.Invoke();

        yield return null;
    }

    //protected Transform heading;

    //// Update is called once per frame
    //protected void FixedUpdate(){
    //	if ( heading!= null && heading.position != Vector3.zero)
    //	{
    //		// heading detected
    //		Vector3 desiredPosition = new Vector3(heading.position.x, transform.position.y, heading.position.z);
    //		Facing(desiredPosition);
    //		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, movementSpeed * Time.deltaTime);
    //		if ((transform.position - heading.position).sqrMagnitude < 1.05)
    //		{
    //			// arrived heading tile
    //			heading = null;
    //		}
    //	}
    //}

    //   // change direction of unit facing, sync with heading
    //void Facing(Vector3 pos)
    //{
    //	transform.LookAt(pos);
    //}

    //public void setHeading(Tile tile)
    //{
    //	//heading is the next tile unit will move to.
    //	heading = tile.transform;
    //}
}
