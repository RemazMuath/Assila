using UnityEngine;
using UnityEngine.UI;  // For UI Image component
using System.Collections;

public class ImageChanger : MonoBehaviour
{
    public Image displayImage;   // The Image component in the Canvas
    public Sprite[] images;      // Array to hold the four images
    public float changeInterval = 1.0f; // Time interval to change images (in seconds)

    private int currentIndex = 0;  // To keep track of the current image index

    void Start()
    {
        if (images.Length > 0 && displayImage != null)
        {
            // Start the image change loop
            StartCoroutine(ChangeImage());
        }
        else
        {
            Debug.LogError("Images or Image component not assigned!");
        }
    }

    // Coroutine to change the image every few seconds
    IEnumerator ChangeImage()
    {
        while (true)
        {
            // Set the current image to the display image
            displayImage.sprite = images[currentIndex];

            // Increment the index to the next image
            currentIndex = (currentIndex + 1) % images.Length;

            // Wait for the specified interval before changing the image
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
