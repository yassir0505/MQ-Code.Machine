# 🛠️ MQ-Code.Machine

**MQ-Code.Machine** is a Final Project (PFE) for the **Grado Superior** diploma. The goal is to simplify machine maintenance and management using **QR Codes**.

## 🔍 Project Idea

When a user scans a QR code attached to a machine, they can:
- View machine type and details
- Check full maintenance history
- Add new maintenance records
- Submit requests to add new machines

## 👥 User Roles & Permissions

| Role            | Permissions                                                 |
|-----------------|-------------------------------------------------------------|
| 👨‍🔧 Technician  | Scan machines + Add maintenance                             |
| 👨‍💼 Team Leader | All Technician rights + Manage team members                 |
| 🧑‍💼 Manager     | Full access + Approve machines + Generate QR + View reports |

## 🧱 Technologies Used

- `ASP.NET Core` with Razor Pages
- `Entity Framework Core` + `PostgreSQL`
- `QRCoder` for generating QR codes
- `iTextSharp` for PDF creation
- `Session-based authentication`

## ✅ Completed Features

- [x] Register/Login system with role-based access
- [x] Machine request and approval workflow
- [x] Generate QR code with PDF download
- [x] Maintenance logging and history
- [x] Manager dashboard with machine status

## 🔜 Upcoming Features

- [ ] Improved database structure to support many users
- [ ] Advanced security & password encryption
- [ ] "Forgot password?" with email reset functionality
- [ ] Convert to Mobile App (future goal)
- [ ] Internal notifications system

## 🌍 Project Link (once deployed)
🔗 Coming soon...

---

> Built with passion ❤️ by [Mohamed (yassir0505)](https://github.com/yassir0505) 👨‍💻
