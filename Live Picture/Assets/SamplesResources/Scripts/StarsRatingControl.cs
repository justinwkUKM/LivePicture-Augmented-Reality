/*============================================================================== 
Copyright (c) 2017 PTC Inc. All Rights Reserved.

 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;

/// <summary>
/// This class handles the state of the 5 stars that are part of the augmentation view.
/// </summary>
public class StarsRatingControl : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public GameObject[] Stars;
    public Material FillTexture;
    public Material EmptyTexture;
    #endregion //PUBLIC_VARIABLES


    #region PRIVATE_MEMBERS
    private int mRating;
    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_METHODS
    /// <summary>
    /// Sets textures to the star object to render the corresponding rating
    /// </summary>
    public void SetRating(int rating )
    {
        mRating = rating;
        
        // Validates that the rating is not bigger than 5
        if (mRating > 5 )mRating = 5;
        
        // Initialize all the stars as empty
        for (int i=0; i < Stars.Length; i++)
        {
            GameObject star = Stars[i];
            star.GetComponent<MeshRenderer>().material = EmptyTexture;
        }
        
        // fills the stars
        for (int i=0; i < mRating; i++)
        {
            GameObject star = Stars[i];
            star.GetComponent<MeshRenderer>().material = FillTexture;
        }
    }
    #endregion //PUBLIC_METHODS
}
