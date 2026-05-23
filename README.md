# 🌤️ Modern Weather Tracker (WinForms)

An elegant, high-performance desktop application built in C# Windows Forms that delivers real-time weather monitoring wrapped in a gorgeous liquid-glass effect (**Glassmorphism**) interface.

## ✨ Key Features

* **Fluid UI Architecture:** Modern desktop interface with translucent cards, custom rounded borders (GDI+ custom rendering), and clean, distraction-free typography.
* **Contextual Visuals:** Completely dynamic user experience powered by the Unsplash Engine, changing the ultra-HD background imagery to match the specific city queried by the user.
* **Asynchronous Processing:** Built using asynchronous non-blocking network I/O (`HttpClient`), ensuring the UI stays completely responsive and smooth during weather data and high-res image lookups.
* **Persistent Caching:** Includes a lightweight automated caching system that logs and retrieves local search history across application lifecycles.

## 🛠️ Built With

* **C#** (Windows Forms Core)
* **GDI+ Graphics Tooling** (SmoothingMode, GraphicsPath API)
* **wttr.in API** (Minimalist REST Weather Feed)
* **Unsplash Media Proxy** (Dynamic Image Sourcing)

## 🚀 Getting Started

### Prerequisites
* Windows 10 / 11 OS
* Visual Studio 2022 (with .NET Desktop Development Workload installed)

### Setup & Execution
1. Clone this repository directly onto your desktop:
   ```bash
   git clone [https://github.com/davidstefan19/Modern-Weather-App.git](https://github.com/davidstefan19/Modern-Weather-App.git)
