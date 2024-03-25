FROM alpine:3.9.4

# Add some libs required by .NET runtime 
# https://github.com/dotnet/core/blob/master/Documentation/build-and-install-rhel6-prerequisites.md#troubleshooting
RUN apk add --no-cache \ 
    openssh libunwind \
    nghttp2-libs libidn krb5-libs libuuid lttng-ust zlib \
    libstdc++ libintl \
    icu

EXPOSE 80
EXPOSE 443

# Copy 
WORKDIR /app
COPY ./bin/Release/netcoreapp3.1/publish ./

ENTRYPOINT ["./DAS.Notify", "--urls", "http://0.0.0.0:80"]