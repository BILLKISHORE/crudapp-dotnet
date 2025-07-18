#!/bin/bash

# Create assets directory structure
mkdir -p assets/images

# Copy image files with new names
cp .prompt_attachments/d0d64a44-b202-43a2-ab48-046599552efd.png assets/images/settings_profile_1.png
cp .prompt_attachments/1f31b1d4-20ab-4560-91b6-06efd4704580.png assets/images/settings_profile_2.png
cp .prompt_attachments/b4306869-a24c-4b1d-92fc-bc0eeff3acc6.png assets/images/settings_profile_3.png

echo "Assets copied successfully!"
echo "Files copied:"
echo "- assets/images/settings_profile_1.png"
echo "- assets/images/settings_profile_2.png"
echo "- assets/images/settings_profile_3.png"