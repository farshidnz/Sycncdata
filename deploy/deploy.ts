#!/usr/bin/env node
import 'source-map-support/register';
import { App, StackProps } from "aws-cdk-lib";
import { getEnv, getResourceName } from '@cashrewards/cdk-lib'
import * as dotenv from 'dotenv'
import { existsSync } from 'fs'
import * as path from 'path'
import { DeployStack } from './lib/deploy-stack';

export class AccountsSyncDataApp extends App {
  protected stackProps: StackProps;
  public deployStack: DeployStack;
  constructor() {
    super();
    this.stackProps = {
      env: {
        account: getEnv("AWS_ACCOUNT_ID"),
        region: getEnv("AWS_REGION"),
      },
    };
    this.deployStack = new DeployStack(
      this,
      getResourceName(getEnv("PROJECT_NAME")),
      this.stackProps
    );
  }
}


(async () => {
  const envFile = path.resolve(process.cwd(), '.env');
  if (existsSync(envFile)) {
    dotenv.config();
  }

  return new AccountsSyncDataApp();
})()