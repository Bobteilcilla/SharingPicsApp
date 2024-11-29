using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PictureUploader
{
    public partial class Form1 : Form
    {
        private string selectedImagePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    pictureBox.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=testingappsbobteilcilla;AccountKey=cccjOI6Mtk7pJALdK8oJ/hMPTkVgqOGoWsOwSlUoxoRKiWauqhT4IjvpxEWENcT+ELPth6GcxLjf+AStlC/dvw==;EndpointSuffix=core.windows.net";
            string containerName = "pictures";

            string blobUrl = UploadFileToBlobStorage(connectionString, containerName, selectedImagePath);

            if (blobUrl != null)
            {
                txtImageUrl.Text = blobUrl;
                MessageBox.Show("Image uploaded successfully!");
            }
            else
            {
                MessageBox.Show("Failed to upload image.");
            }
        }

        private string UploadFileToBlobStorage(string connectionString, string containerName, string localFilePath)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists(PublicAccessType.Blob);

                string fileName = Path.GetFileName(localFilePath);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using (FileStream uploadFileStream = File.OpenRead(localFilePath))
                {
                    blobClient.Upload(uploadFileStream, true);
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return null;
            }
        }

        
    }
}
