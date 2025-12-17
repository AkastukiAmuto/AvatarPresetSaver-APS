const fs = require('fs');
const https = require('https');
const path = require('path');

// Configuration
const REPO_URL = `https://${process.env.GITHUB_REPOSITORY_OWNER}.github.io/${process.env.GITHUB_EVENT_REPOSITORY_NAME}/index.json`;
const REPO_NAME = process.env.GITHUB_EVENT_REPOSITORY_NAME;
const REPO_AUTHOR = process.env.GITHUB_REPOSITORY_OWNER;

async function main() {
    // 1. Read package.json
    const packageJson = JSON.parse(fs.readFileSync('package.json', 'utf8'));
    const version = packageJson.version;

    // 2. Fetch existing index.json (if any)
    let repo = {
        name: REPO_NAME,
        author: REPO_AUTHOR,
        url: REPO_URL,
        packages: {}
    };

    try {
        const data = fs.readFileSync('index.json', 'utf8');
        const loaded = JSON.parse(data);
        // Merge loaded data with defaults or just use it, but ensure packages exists
        if (loaded && typeof loaded === 'object') {
            repo = { ...repo, ...loaded };
        }
    } catch (e) {
        console.log("No local index.json found or invalid, creating new one.");
    }

    // Ensure packages object exists
    if (!repo.packages) {
        repo.packages = {};
    }

    // 3. Construct Package Entry
    // We assume the user attached a zip named "AvatarPresetSaver_v1.0.0.unitypackage" or similar? 
    // Actually for VCC, it needs a zip of the PACKAGE FOLDER, not unitypackage.
    // GitHub automatically provides source tarballs. 
    // url: https://github.com/USER/REPO/archive/refs/tags/v1.0.0.zip is problematic because of root folder.

    // Simplest path: Use the standard "zipball" but VCC handles it fine usually?
    // Proper way: Release workflow should zip the content.

    // Let's assume we simply point to the source zip for now.
    // Use the uploaded asset zip
    // URL format: https://github.com/USER/REPO/releases/download/TAG/FILENAME.zip
    // We use the Tag Name explicitly to match the Release URL structure
    const tagName = process.env.TAG_NAME;
    const zipName = process.env.ZIP_NAME;
    const downloadUrl = `https://github.com/${process.env.GITHUB_REPOSITORY}/releases/download/${tagName}/${zipName}`;

    const packageEntry = {
        ...packageJson,
        url: downloadUrl,
        repo: REPO_URL
    };

    // 4. Update Repo
    if (!repo.packages[packageJson.name]) {
        repo.packages[packageJson.name] = { versions: {} };
    }
    repo.packages[packageJson.name].versions[version] = packageEntry;

    // 5. Write index.json
    fs.writeFileSync('index.json', JSON.stringify(repo, null, 2));
    console.log("Updated index.json");
}

main();
