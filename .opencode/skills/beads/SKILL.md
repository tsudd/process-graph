---
name: beads-tracking
description: Beads is a git-backed, AI-native issue tracking system. This skill helps AI agents work with Beads effectively.
--- 


## When to Use
This skill is essential for managing tasks and issues in the project. Use it whenever you need to:
- Find available work or tasks to complete.
- Create new issues for tracking bugs, features, or follow-ups.
- Update the status of ongoing work.
- Close issues when work is completed.
- Synchronize the issue tracker with the Git repository.

## Quick Reference: Creating Issues
To create a new issue, follow these steps:
1. Run the `bd onboard` command to initialize the beads tool if not already set up.
2. Use the `bd new` command to create an issue. Provide a descriptive title and details about the task.
   ```bash
   bd new "Title of the issue" --description "Detailed description of the issue."
   ```
3. Assign labels, priorities, or other metadata as needed using the `--labels` or `--priority` flags.
4. Sync the issue tracker with Git to ensure the issue is properly recorded:
   ```bash
   bd sync
   ```

## Resources
