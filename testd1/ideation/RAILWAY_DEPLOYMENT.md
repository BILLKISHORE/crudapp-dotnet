# Railway Deployment Guide

## 🚀 Deploying School Exam Webapp on Railway

This guide will help you deploy your full-stack school exam management system on Railway.

### Prerequisites

1. **Railway Account**: Sign up at [railway.app](https://railway.app)
2. **GitHub Repository**: Push your code to GitHub
3. **API Keys**: Gather all required API keys (see environment variables section)

### Step 1: Deploy Backend (FastAPI)

1. **Create New Project on Railway**
   - Go to [railway.app](https://railway.app)
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository

2. **Configure Backend Service**
   - Railway will detect your Python application
   - Set the **Root Directory** to: `testv1`
   - Set the **Build Command** to: `pip install -r requirements.txt`
   - Set the **Start Command** to: `python -m uvicorn Backend.main:app --host 0.0.0.0 --port $PORT`

3. **Add Database Services**
   - In your Railway project, click "Add Service"
   - Add **PostgreSQL** database
   - Add **Redis** for caching and background jobs

4. **Configure Environment Variables**
   Copy these environment variables to your Railway service:

   ```env
   # Application Settings
   ENVIRONMENT=production
   DEBUG=false
   APP_NAME=School Exam Webapp
   APP_VERSION=1.0.0

   # Database (Railway provides these automatically)
   DATABASE_URL=${{Postgres.DATABASE_URL}}
   
   # Redis (Railway provides these automatically)
   REDIS_URL=${{Redis.REDIS_URL}}

   # JWT Configuration
   SECRET_KEY=your-super-secret-key-here
   ALGORITHM=HS256
   ACCESS_TOKEN_EXPIRE_MINUTES=60

   # CORS Settings (update with your frontend domain)
   ALLOWED_ORIGINS=https://your-frontend.railway.app,https://localhost:3000

   # Supabase Configuration
   SUPABASE_URL=your-supabase-url
   SUPABASE_KEY=your-supabase-anon-key
   SUPABASE_SERVICE_KEY=your-supabase-service-key

   # AI Service API Keys
   OPENAI_API_KEY=your-openai-api-key
   CLAUDE_API_KEY=your-claude-api-key
   GEMINI_API_KEY=your-gemini-api-key

   # Email Service
   SENDGRID_API_KEY=your-sendgrid-api-key

   # File Upload Settings
   MAX_FILE_SIZE=10485760
   ALLOWED_FILE_TYPES=.pdf,.doc,.docx,.txt,.jpg,.jpeg,.png
   ```

### Step 2: Deploy Frontend (React)

1. **Add Frontend Service**
   - In your Railway project, click "Add Service"
   - Select "Deploy from GitHub repo"
   - Choose the same repository
   - Set the **Root Directory** to: `testv1/frontend/srichai-v2-main`

2. **Configure Frontend Service**
   - Railway will detect your Node.js application
   - Set the **Build Command** to: `npm run build`
   - Set the **Start Command** to: `npm run preview -- --host 0.0.0.0 --port $PORT`

3. **Add Frontend Environment Variables**
   ```env
   # Backend API URL (update with your backend domain)
   VITE_API_URL=https://your-backend.railway.app
   
   # Supabase Configuration
   VITE_SUPABASE_URL=your-supabase-url
   VITE_SUPABASE_ANON_KEY=your-supabase-anon-key
   ```

### Step 3: Configure Service Communication

1. **Update Backend CORS Settings**
   - In your backend service environment variables
   - Update `ALLOWED_ORIGINS` to include your frontend Railway domain:
     ```
     ALLOWED_ORIGINS=https://your-frontend.railway.app,https://localhost:3000
     ```

2. **Update Frontend API URL**
   - In your frontend service environment variables
   - Set `VITE_API_URL` to your backend Railway domain:
     ```
     VITE_API_URL=https://your-backend.railway.app
     ```

### Step 4: Database Initialization

After deployment, you'll need to initialize your database:

1. **Access Railway CLI**
   ```bash
   npm install -g @railway/cli
   railway login
   railway connect
   ```

2. **Initialize Database**
   ```bash
   railway run python -m Backend.scripts.init_db
   ```

### Step 5: Verify Deployment

1. **Check Backend Health**
   - Visit: `https://your-backend.railway.app/health`
   - Should return: `{"status": "healthy"}`

2. **Check Frontend**
   - Visit: `https://your-frontend.railway.app`
   - Should load the application interface

3. **Test API Documentation**
   - Visit: `https://your-backend.railway.app/docs`
   - Should show the FastAPI Swagger UI

### Troubleshooting

#### Common Issues:

1. **Database Connection Error**
   - Verify `DATABASE_URL` is set correctly
   - Check PostgreSQL service is running
   - Ensure database is initialized

2. **CORS Error**
   - Verify `ALLOWED_ORIGINS` includes your frontend domain
   - Check frontend `VITE_API_URL` points to backend

3. **Environment Variables**
   - Verify all required API keys are set
   - Check Railway service references (e.g., `${{Postgres.DATABASE_URL}}`)

4. **Build Failures**
   - Check Railway build logs
   - Verify all dependencies are in requirements.txt/package.json

### Production Optimizations

1. **Custom Domains**
   - Add custom domains in Railway project settings
   - Update environment variables accordingly

2. **Monitoring**
   - Enable Railway's built-in monitoring
   - Set up alerts for service health

3. **Scaling**
   - Monitor resource usage
   - Scale services based on traffic

4. **Security**
   - Ensure all API keys are properly secured
   - Use Railway's environment variable encryption

### Cost Considerations

- **Starter Plan**: $5/month includes:
  - 512MB RAM, 1 vCPU
  - 1GB storage
  - Suitable for development/testing

- **Pro Plan**: $20/month includes:
  - 8GB RAM, 8 vCPU
  - 100GB storage
  - Suitable for production

### Support

- **Railway Documentation**: [docs.railway.app](https://docs.railway.app)
- **Discord Community**: [railway.app/discord](https://railway.app/discord)
- **Support**: [railway.app/help](https://railway.app/help) 