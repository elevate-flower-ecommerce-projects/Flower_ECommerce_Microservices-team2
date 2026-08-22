# 📘 Flower E-Commerce Microservices (Team 2) - CI/CD Pipeline Guidebook

Welcome to the official **CI/CD Pipeline Guidebook** for Team 2's Flower E-Commerce Microservices project.

---

## 📑 Table of Contents
1. [Chapter 1: Overview & Architecture](#chapter-1-overview--architecture)
2. [Chapter 2: Step-by-Step Pipeline Creation](#chapter-2-step-by-step-pipeline-creation)
3. [Chapter 3: Prerequisites & Secrets Setup (Docker Hub & GitHub)](#chapter-3-prerequisites--secrets-setup-docker-hub--github)
4. [Chapter 4: Deep Dive into the CI/CD Workflow File](#chapter-4-deep-dive-into-the-cicd-workflow-file)
5. [Chapter 5: Troubleshooting & Best Practices](#chapter-5-troubleshooting--best-practices)

---

## Chapter 1: Overview & Architecture

### Purpose
The CI/CD pipeline automates image compilation and publishing for Team 2's microservices platform. Every time code is pushed to `main`, `master`, or `Development`, GitHub Actions automatically:
- Builds container images for all 7 Team 2 microservices in parallel.
- Pushes Docker images to **Docker Hub** (`soloprogrammer88`) using matrix tag names.

### Team 2 Microservices Map

| Service Name | Dockerfile Location | Docker Hub Image Name |
| :--- | :--- | :--- |
| **API Gateway** | `./API Gateway/Dockerfile` | `flower-apigateway-team2` |
| **Auth Service / Identity** | `./AuthService/Dockerfile` | `flower-identity-service-team2` |
| **Address & Store Coverage** | `./Address & Store Coverage Service/Dockerfile` | `flower-address-service-team2` |
| **Cart Service** | `./Cart ServiceCart Service/Dockerfile` | `flower-cart-service-team2` |
| **Catalog Service** | `./Catalog Service/Dockerfile` | `flower-catalog-service-team2` |
| **Order & Fulfillment Service** | `./Order & Fulfillment Service/Dockerfile` | `flower-order-service-team2` |
| **Payment Service** | `./Payment Service/Dockerfile` | `flower-payment-service-team2` |

---

## Chapter 2: Prerequisites & Secrets Setup (Docker Hub & GitHub)

### Step 1: GitHub Repository Secrets Configuration
1. Go to Team 2's GitHub repository settings (**Settings > Secrets and variables > Actions**).
2. Add the following secrets:
   - `DOCKER_USERNAME`: `soloprogrammer88`
   - `DOCKER_PASSWORD`: `<YOUR_DOCKER_HUB_PERSONAL_ACCESS_TOKEN>`

---

## Chapter 3: Workflow Configuration

Workflow file located at: [.github/workflows/ci-cd.yml](file:///d:/partition%20h/Elevate/flower%20ecommerce/team2/Flower_ECommerce_Microservices-team2/.github/workflows/ci-cd.yml)
