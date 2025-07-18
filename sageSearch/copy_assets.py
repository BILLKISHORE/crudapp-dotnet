#!/usr/bin/env python3
"""
Script to copy image assets from .prompt_attachments to assets/images/
"""

import os
import shutil

def main():
    # Create assets directory structure
    os.makedirs('assets/images', exist_ok=True)
    
    # Define source to destination mapping
    file_mappings = {
        '.prompt_attachments/d0d64a44-b202-43a2-ab48-046599552efd.png': 'assets/images/settings_profile_1.png',
        '.prompt_attachments/1f31b1d4-20ab-4560-91b6-06efd4704580.png': 'assets/images/settings_profile_2.png',
        '.prompt_attachments/b4306869-a24c-4b1d-92fc-bc0eeff3acc6.png': 'assets/images/settings_profile_3.png'
    }
    
    print("Copying image assets...")
    
    for source, destination in file_mappings.items():
        if os.path.exists(source):
            shutil.copy2(source, destination)
            print(f"✅ Copied: {source} → {destination}")
        else:
            print(f"❌ Source file not found: {source}")
    
    print("\nAssets copied successfully!")
    print("Files should now be available at:")
    for dest in file_mappings.values():
        print(f"- {dest}")

if __name__ == "__main__":
    main()