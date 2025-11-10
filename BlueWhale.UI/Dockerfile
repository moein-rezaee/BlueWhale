# Build stage
FROM node:20-alpine AS builder
WORKDIR /builder

COPY package*.json ./
RUN npm ci

COPY . .
RUN ls -la assets/css/
RUN npm run build

# Production stage
FROM node:20-alpine
WORKDIR /app

# Copy runtime dependencies
COPY package.json package-lock.json ./
RUN npm ci --omit=dev

# Copy built artifacts from builder
COPY --from=builder /builder/.output ./.output
COPY --from=builder /builder/.nuxt ./.nuxt
COPY --from=builder /builder/public ./public
COPY --from=builder /builder/node_modules/.bin ./node_modules/.bin

# Create data directory for persistence
RUN mkdir -p /app/data

EXPOSE 3000

ENV HOST=0.0.0.0 \
    PORT=3000 \
    NODE_ENV=production \
    NUXT_PUBLIC_API_BASE=http://registry-api:5260/v1/api \
    NUXT_PUBLIC_REGISTRY_URL=http://registry:5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD node -e "require('http').get('http://localhost:3000', (r) => {if (r.statusCode !== 200) throw new Error(r.statusCode)})" || exit 1

CMD ["node", ".output/server/index.mjs"]
