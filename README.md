## Tech Stack

- **Backend**: .NET Framework (8.0.4+)
- **Database**: PostgreSQL
- **Testing**: xUnit

## General Development Workflow

### For All Team Members:

1. **Start Working on Feature**

```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

2. **Daily Workflow**

```bash
# Before starting work each day (to make sure you have any changes in main)
git checkout main
git pull origin main
git checkout feature/your-feature-name
git merge main  # Get latest changes

# After making changes
git add .
git commit -m "Descriptive commit message"
git push origin feature/your-feature-name
```